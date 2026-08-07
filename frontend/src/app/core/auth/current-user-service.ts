import { HttpClient } from '@angular/common/http';
import { computed, inject, Service, signal } from '@angular/core';
import { UserResponse } from '../../shared/dtos/user.dtos';
import { firstValueFrom } from 'rxjs';

/* Hält die Stammdaten des angemeldeten Benutzers */
@Service()
export class CurrentUserService {
    private readonly http: HttpClient = inject(HttpClient);
    private readonly _user = signal<UserResponse | null>(null);

    readonly user = this._user.asReadonly();

    /** Vorname für die persönliche Ansprache, der Anzeigename kann mehrteilig sein */
    readonly firstName = computed(() => {
        const displayName = this._user()?.displayName.trim();

        if(!displayName){
            return null;
        }

        return displayName.split(" ")[0];
    });

    /** Benutzerdaten von der API laden */
    async load(): Promise<void> {
        const user: UserResponse = await firstValueFrom(
            this.http.get<UserResponse>("/api/me")
        );
        this._user.set(user);
    }
}
