## Next steps

{% assign subPages = site.pages
    | where: "parent", page.title
    | sort: "order" %}

{% for subPage in subPages %}

- ### [{{subPage.title}}]({{subPage.url | replace: '.html', '' | relative_url}})
{% endfor %}