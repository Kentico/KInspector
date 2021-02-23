<template>
    <div v-if="hasCoreVersion">
        KenticoInspector.Core Version {{coreVersion}}
    </div>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex'
    export default {
        mounted() {
            this.getCoreVersion();
        },
        computed: {
            ...mapGetters('versions', [
                'coreVersion'
            ]),
            hasCoreVersion: function (){
                return this.coreVersion != "";
            }
        },
        methods: {
            ...mapActions('versions', ['getCoreVersion']),
        }

    }
</script>