import LocalSessionStorage from "@js/stores/localSessionStorage";
import type IRequestInterceptor from "@js/api/interceptors/request/iRequestInterceptor";
import InterceptorResponse from "@js/api/interceptors/interceptorResponse";

/**
 * Interceptor to perform an update of the server session at each request
 */
export default class RefreshClientSessionRequestInterceptor implements IRequestInterceptor {
    async run(_url: string, _request?: RequestInit): Promise<InterceptorResponse> {
        //Updating $lastServerRequestDate each time a new request is made
        LocalSessionStorage.updateLastServerRequestDate();

        return new InterceptorResponse(true, false);
    }
}