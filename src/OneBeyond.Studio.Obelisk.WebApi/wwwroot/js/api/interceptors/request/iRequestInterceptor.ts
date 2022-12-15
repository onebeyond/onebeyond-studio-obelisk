import InterceptorResponse from "@js/api/interceptors/interceptorResponse";

export default interface IRequestInterceptor {
    /**
     * Run the interceptor
     * @param url the URL of the fetch call
     * @param request the Request of the fetch call
     * @returns a composite response @see InterceptorResponse
     */
    run(url: string, request?: RequestInit): Promise<InterceptorResponse>;
}