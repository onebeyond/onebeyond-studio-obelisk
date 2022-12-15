import { DateTimeFormats } from "vue-i18n"

const dateTimeFormats: DateTimeFormats = {
    en: {
        short: {
            year: 'numeric', month: 'short', day: 'numeric'
        },
        long: {
            year: 'numeric', month: 'long', day: 'numeric',
            hour: 'numeric', minute: 'numeric'
        }
    }
}

export default dateTimeFormats