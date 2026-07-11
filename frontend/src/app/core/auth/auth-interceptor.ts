import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { TokenStoreService } from "./token-store-service";

/** Hängt das Access Token als Bearer Header an ausgehende Requests. */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const accessToken = inject(TokenStoreService).accessToken();

    if(accessToken === null){
        return next(req);
    }

    return next(req.clone({
        setHeaders: {
            Authorization: `Bearer ${accessToken}`
        },
    }));
};