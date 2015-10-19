using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Helpers;
using System.Data;
using System.IO;


namespace Kentico.KInspector.Modules
{
    /// <summary>
    /// Prints auditing information about the instance.
    /// Designed to be used with KInspector auditing tool.
    /// </summary>
    /// <remarks>
    /// The probe is a web form designed to work in both web site
    /// and web application environment. Moreover it has to work
    /// with different Kentico versions.
    /// From future perspective it could consist of multiple controls.
    /// </remarks>
    public partial class KInspectorProbe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable table = GetCacheItems();
            Response.ContentType = "text/xml";
            table.WriteXml(Response.Output, XmlWriteMode.WriteSchema);
            Response.End();
        }


        private DataTable GetCacheItems()
        {
            lock (HttpRuntime.Cache)
            {
                List<string> keyList = new List<string>();
                IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();

                // Build the items list
                while (cacheEnum.MoveNext())
                {
                    string key = cacheEnum.Key.ToString();
                    if (!String.IsNullOrEmpty(key))
                    {
                        keyList.Add(key);
                    }
                }
                keyList.Sort();

                // Load the grid
                return GetCacheItemsData(keyList);
            }
        }


        /// <summary>
        /// Prints cache items data to a table.
        /// </summary>
        /// <param name="cacheItems">Cache items to be printed.</param>
        private DataTable GetCacheItemsData(IEnumerable<string> cacheItems)
        {
            DataTable table = new DataTable("CacheItems");
            table.Columns.Add("Key");
            table.Columns.Add("Data");
            table.Columns.Add("Size");
            table.Columns.Add("Expiration");
            table.Columns.Add("Priority");

            if (cacheItems != null)
            {
                // Process dummy keys
                foreach (string key in cacheItems)
                {

                    if (!String.IsNullOrEmpty(key))
                    {
                        // Process the key
                        object value = HttpRuntime.Cache[key];
                        CacheItemContainer container = null;

                        // Handle the container
                        if (value is CacheItemContainer)
                        {
                            container = (CacheItemContainer)value;
                            value = container.Data;
                        }

                        if (value != CacheHelper.DUMMY_KEY)
                        {
                            PrintCacheItemData(table, key, container, value);
                        }
                    }
                }
            }

            return table;
        }


        /// <summary>
        /// Renders the particular cache item.
        /// </summary>
        protected void PrintCacheItemData(DataTable table, string key, CacheItemContainer container, object value)
        {
            if (key.Length > 100)
            {
                key = key.Substring(0, 100);
            }

            int size = 0;
            if ((value != null) && (value != DBNull.Value))
            {
                value = HttpUtility.HtmlEncode(DataHelper.GetObjectString(value, 100, out size));
            }

            object expiration = null;
            object priority = null;

            if (CacheDebug.Settings.Enabled)
            {
                if (container != null)
                {
                    if (container.AbsoluteExpiration != Cache.NoAbsoluteExpiration)
                    {
                        expiration = container.AbsoluteExpiration;
                    }
                    else
                    {
                        expiration = container.SlidingExpiration;
                    }

                    priority = container.Priority;
                }
            }

            table.Rows.Add(key, value, size, expiration, priority);
        }
    }
}