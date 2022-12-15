import type IResponseInterceptor from "@js/api/interceptors/response/iResponseInterceptor";
import InterceptorResponse from "@js/api/interceptors/interceptorResponse";

/**
 * Interceptor to perform logout and redirect to Login page in case of 401
 */
export default class RedirectToLoginResponseInterceptor implements IResponseInterceptor {
    private readonly logoutCallback: Function;

    constructor(logoutCallback: Function) {
        this.logoutCallback = logoutCallback;
    }

    async run(response: Response, _request: () => Promise<Response>): Promise<InterceptorResponse> {
        if (response.status === 401) {
            try {
                await this.logoutCallback.call(this);
            }
            catch {
                // Avoid issues in case the user has been already logged out
            }
            window.location.href = `${(window as any).location.origin}/auth/`;
            return new InterceptorResponse(false, false);
        }

        return new InterceptorResponse(true, false);
    }
}