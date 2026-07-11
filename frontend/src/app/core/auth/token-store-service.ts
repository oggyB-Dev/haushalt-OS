import { Service, signal } from '@angular/core';

const REFRESH_TOKEN_KEY = "haushaltsos.refreshToken";


/**
 * Verwaltet die Auth Tokens gemäß ADR Entscheidung:
 * Access Token nur im Speicher (Signal), Refresh Token in localStorage
 */
@Service()
export class TokenStoreService {
    /** Access Token bewusst nur im Speicher, überlebt kein Reload */
    private readonly _accessToken = signal<string | null>(null);

    /** Lesender Zugriff auf das Access Token */
    readonly accessToken = this._accessToken.asReadonly();

    /** Speichert ein neues Tokenpaar */
    setTokens(accessToken: string, refreshToken: string) : void {
        this._accessToken.set(accessToken);
        localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
    }

    /** Liest das gespeicherte Refresh Token */
    getRefreshToken(): string | null {
        return localStorage.getItem(REFRESH_TOKEN_KEY);
    }

    /** Entfernt alle Tokens = Logout */
    clear(): void {
        this._accessToken.set(null);
        localStorage.removeItem(REFRESH_TOKEN_KEY);
    }
}
