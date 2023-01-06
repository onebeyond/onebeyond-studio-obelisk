<template>
    <td class="hide-on-mobile">
        <div>
            <slot />
        </div>
    </td>
</template>

<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";

    @Component({
        name: "dcsl-inline-grid-td"
    })
    export default class dcslInlineGridTd extends Vue {
        constructor() { super(); }
    }
</script>


