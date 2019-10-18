---
title: Glossary
mainNavigation: true
order: 100
---

# Glossary

The Kentico Inspector solution contains references and concepts with special and consistent meaning. It is recommended to be familiar with them when discussing Kentico Inspector in an [issue](https://github.com/Kentico/KInspector/issues/new) or a [pull request](https://help.github.com/articles/using-pull-requests/).

{% assign glossary = site.pages
    | where_exp: "item", "item.dir == '/glossary/'"
    | sort: "name"
    | group_by_exp: "item", "item.name
        | slice: 0
        | capitalize"
    | sort: "name" %}

{% for group in glossary %}

### {{group.name}}

{% for item in group.items %}

#### {{item.title}}

{{item.content }}
{% endfor %}
{% endfor %}
