import type IResponseInterceptor from "@js/api/interceptors/response/iResponseInterceptor";
import InterceptorResponse from "@js/api/interceptors/interceptorResponse";

/**
 * Interceptor to redirect to PageNotFound in case of 403
 */
export default class RedirectToNotFoundResponseInterceptor implements IResponseInterceptor {
    async run(response: Response, _request: () => Promise<Response>): Promise<InterceptorResponse> {
        if (response.status === 403) {
            window.location.href = `${(window as any).location.origin}/#/*`;
            return new InterceptorResponse(false, false);
        }

        return new InterceptorResponse(true, false);
    }
}