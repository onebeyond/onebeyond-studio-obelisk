<template>
    <tr>
        <slot></slot>
    </tr>
</template>

<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";

    @Component({
        name: "dcsl-inline-grid-tr"
    })
    export default class dcslInlineGridTr extends Vue {
        constructor() { super(); }
    }
</script>


