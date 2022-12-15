import { i18n } from '@js/localizations/lang';

// VueTable settings
const vueTableLocale = {
    texts: {
        get count() { return i18n.t('vueTables.count'); },
        get filter() { return i18n.t('vueTables.filter'); },
        get filterPlaceholder() { return i18n.t('vueTables.filterPlaceholder'); },
        get limit() { return i18n.t('vueTables.limit'); },
        get page() { return i18n.t('vueTables.page'); },
        get noResults() { return i18n.t('vueTables.noResults'); },
        get filterBy() { return i18n.t('vueTables.filterBy'); },
        get loading() { return i18n.t('vueTables.loading'); },
        get defaultOption() { return i18n.t('vueTables.defaultOption'); }
    },
    sortIcon: {
        base: "fas",
        up: "fa-sort-up",
        down: "fa-sort-down",
        is: "fa-sort"
    },
    pagination: {
        edge: true
    }
};

export { vueTableLocale }