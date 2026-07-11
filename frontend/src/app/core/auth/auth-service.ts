import { HttpClient } from '@angular/common/http';
import { computed, inject, Service } from '@angular/core';
import { TokenStoreService } from './token-store-service';
import { AuthResponse, LoginRequest } from '../../shared/dtos/auth.dtos';
import { firstValueFrom } from 'rxjs';

/**Kapselt Login, Logout und Session Wiederherstellung gegen die Auth api */
@Service()
export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly tokenStoreService = inject(TokenStoreService);

    private readonly apiUrl = "http://localhost:5237";

    /**Prüfen ob der Benutzer aktuell angemeldet ist */
    readonly isAuthenticated = computed(() => 
        this.tokenStoreService.accessToken() !== null
    );

    /** Meldet den Benutzer an und speichert das Tokenpaar */
    async login(request: LoginRequest) : Promise<void> {
        const response = await firstValueFrom(
            this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, request)
        );

        this.tokenStoreService.setTokens(response.accessToken, response.refreshToken);
    }

    /** Meldet den Benutzer ab */
    logout(): void{
        this.tokenStoreService.clear();
    }
}
