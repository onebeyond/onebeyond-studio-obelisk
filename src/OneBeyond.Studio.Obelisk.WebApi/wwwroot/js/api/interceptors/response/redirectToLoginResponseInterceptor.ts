import type IResponseInterceptor from "@js/api/interceptors/response/iResponseInterceptor";
import InterceptorResponse from "@js/api/interceptors/interceptorResponse";

/**
 * Interceptor to perform logout and redirect to Login page in case of 401 or 302
 */
export default class RedirectToLoginResponseInterceptor implements IResponseInterceptor {
    private readonly logoutCallback: Function;

    constructor(logoutCallback: Function) {
        this.logoutCallback = logoutCallback;
    }

    async run(response: Response, _request: () => Promise<Response>): Promise<InterceptorResponse> {
        if (response.status === 401 || response.redirected) {
            await this.logoutCallback.call(this);
            window.location.href = `${(window as any).BaseUrl}Account/Login`;
            return new InterceptorResponse(false, false);
        }

        return new InterceptorResponse(true, false);
    }
}
