import InterceptorResponse from "@js/api/interceptors/interceptorResponse";

export default interface IResponseInterceptor {
    /**
     * Run the interceptor
     * @param response the response given by the fetch execution
     * @param request the Request function that had been executed
     * @returns a composite response @see InterceptorResponse
     */
    run(response: Response, request: () => Promise<Response>): Promise<InterceptorResponse>;
}