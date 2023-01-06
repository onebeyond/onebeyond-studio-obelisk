const Vue = require("vue"); // eslint-disable-line @typescript-eslint/no-var-requires
import VeeValidate from "vee-validate";
import SessionTimeoutComponent from "@components/dcslcomponents/sessiontimeout.vue";
import UserContextSetter from "@components/dcslcomponents/usercontextsetter.vue";
import ModalPopup from "@components/util/modalpopup.vue";
import 'regenerator-runtime/runtime'

Vue.use(VeeValidate);
Vue.component("session-timeout", SessionTimeoutComponent);
Vue.component("user-context", UserContextSetter);
Vue.component("v-modalPopup", ModalPopup);
