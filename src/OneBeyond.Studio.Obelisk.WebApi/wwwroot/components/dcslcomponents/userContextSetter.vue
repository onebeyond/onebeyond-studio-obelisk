<template>
    <div></div>
</template>

<script lang="ts">
    import { Vue, Component, Prop } from "vue-property-decorator"

    @Component({
        name: "UserContextSetter",
        components: {}
    })
    export default class UserContextSetter extends Vue {
        @Prop({ required: true, type: String }) userContextJsonString!: string;

        constructor() {
            super();
        }

        created(): void {
            this.$store.commit('setUserContext', this.userContextJsonString)
        }
    }
</script>
