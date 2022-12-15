//Element UI imports required for locale change
import locale from "element-ui/lib/locale";
import elEN from "element-ui/lib/locale/lang/en";

const elementUILocale = {
    use(currentLocale: string) {
        switch (currentLocale) {
            default:
                locale.use(elEN);
        }
    }
}

export { elementUILocale };