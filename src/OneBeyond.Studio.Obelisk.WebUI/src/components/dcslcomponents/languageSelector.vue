<template>
    <div>
        <select v-model="$i18n.locale" @change="changeLanguage($event)" class="form-control">
            <option
                v-for="locale in $i18n.availableLocales"
                :key="locale"
                :value="locale"
            >
                {{$i18n.messages[locale]['application']['language']}}
            </option>
        </select>
    </div>
</template>

<script lang="ts">
    import { Vue, Component } from "vue-property-decorator"
    import { i18nConfig } from '@js/localizations/i18nConfig';
    import LocalAppStorage from "@js/stores/localAppStorage";

    @Component({
        name: "LanguageSelector",
        components: {
        }
    })
    export default class LanguageSelector extends Vue {

        constructor() {
            super();
        }

        public changeLanguage(event): void {
            if (event.target.value !== LocalAppStorage.getValueForKey("currentLocale")) {
                i18nConfig.use(event.target.value);
            }
        }
    }
</script>
