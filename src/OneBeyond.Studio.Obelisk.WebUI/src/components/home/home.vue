<template>
    <div>
        <div class="card-deck-wrapper">
            <div class="card-deck m-5">
                <div class="card p-5">
                    <a class="card-block stretched-link text-decoration-none" :href="adminUrl">
                        <h1 class="card-body text-center">
                            <i class="fas fa-user-cog"></i>
                            {{ $t('admin') }}
                        </h1>
                    </a>
                </div>
                <div class="card p-5">
                    <a
                        class="card-block stretched-link text-decoration-none"
                        href="https://dev.azure.com/dcslsoftwareltd/Wiki/_wiki/wikis/Wiki.wiki/3/Home"
                        target="_blank"
                    >
                        <h1 class="card-body text-center">
                            <i class="fas fa-book"></i>
                            {{ $t('wiki') }}
                        </h1>
                    </a>
                </div>
            </div>
        </div>

        <div class="card p-3 w-50 mx-auto">
            <div class="card-block">
                <h5
                    class="card-title text-center"
                >Do you want to jump to the Admin portal without showing this page?</h5>
                <div class="card-text text-center">
                    Just go to @js/routes/indexRoutes.ts and enable the commented code under the 'Home' route.<br/>
                    Then you can comment out or remove the 'component' property as it won't be neeeded.
                </div>
            </div>
        </div>
    </div>
</template>


<script lang="ts">
    import { Vue, Component } from "vue-property-decorator";
    import dictionary from "@js/localizations/resources/components/home";
    import LocalSessionStorage from "@js/stores/localSessionStorage";

    @Component({
        name: "Home",
        i18n: {
            messages: dictionary
        }
    })
    export default class Home extends Vue {
        constructor() {
            super();
        }

        get adminUrl(): string {
            return LocalSessionStorage.isUserAuthenticated() ? "/admin/" : "/auth/";
        }
    }
</script>
