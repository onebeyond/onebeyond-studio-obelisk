<template>
    <div id="page-not-found" class="text-center">

        <h1>404</h1>
        <h2>{{ $t('notifications.pageNotFound') }}</h2>

        <router-link to="/" class="btn btn-primary btn-go-to-home">{{ $t('button.goToHome') }}</router-link>

    </div>
</template>

<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";

    @Component({
        name: "PageNotFound",
    })
    export default class PageNotFound extends Vue {
        constructor() {
            super();
        }

        created(): void { };
    }
</script>
