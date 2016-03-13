using System;
using System.Collections.Generic;
using System.Data;

namespace Kentico.KInspector.Core
{
    /// <summary>
    /// Results of the module that are used in the <see cref="IModule.GetResults"/> method.
    /// </summary>
    public class ModuleResults
    {
        private dynamic mResult;

        /// <summary>
        /// Type of the results <see cref="ModuleResultsType"/>
        /// </summary>
        public ModuleResultsType ResultType
        {
            get;
            private set;
        }

        /// <summary>
        /// Supports <see cref="string"/>, <see cref="List{string}"/>, <see cref="DataTable"/>, 
        /// <see cref="DataSet"/>, <see cref="List{DataTable}"/>.
        /// </summary>
        public dynamic Result
        {
            get
            {
                return mResult;
            }
            set
            {
                mResult = value;
                if (mResult is DataTable)
                {
                    ResultType = ModuleResultsType.Table;
                }
                else if (mResult is DataSet || mResult is List<DataTable>){
                    ResultType = ModuleResultsType.ListOfTables;
                }
                else if (mResult is List<string>){
                    ResultType = ModuleResultsType.List;
                }
                else if (mResult is string)
                {
                    ResultType = ModuleResultsType.String;
                }
                else
                {
                    throw new NotSupportedException("Given result's type is not supported!");                 
                }
            }
        }


        /// <summary>
        /// Result status.
        /// </summary>
        public Status Status;


        /// <summary>
        /// Comments where should be displayed what should be fixed and how.
        /// </summary>
        public string ResultComment;


        /// <summary>
        /// Indicates that encoding of <see cref="Result"/> can be safely omitted.
        /// Supported only for <see cref="string"/> and <see cref="List{string}"/> result.
        /// </summary>
        public bool Trusted;
    }
}
