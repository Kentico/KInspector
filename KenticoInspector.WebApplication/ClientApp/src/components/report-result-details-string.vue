<template>
  <div>
    <vue-showdown :markdown="string">
    </vue-showdown>
  </div>
</template>

<script>
  export default {
    props: {
      label: {
        type: String,
        default: ""
      },
      string: {
        type: String,
        default: ""
      }
    }
  }
</script>