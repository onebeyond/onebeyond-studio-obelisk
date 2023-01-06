// Vee-Validate import required for locale change
import VeeValidate from "vee-validate";
import vvEN from "vee-validate/dist/locale/en";

const veeValidateLocale = {
    use(currentLocale: string) {
        switch (currentLocale) {
        default:
            VeeValidate.Validator.localize("en", vvEN);
        }
    }
}

export { veeValidateLocale };