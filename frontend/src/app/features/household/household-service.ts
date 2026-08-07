import { HttpClient } from '@angular/common/http';
import { inject, Service, signal } from '@angular/core';
import { HouseholdResponse } from '../../shared/dtos/household.dtos';
import { firstValueFrom } from 'rxjs';

/* Lädt die Haushaltsinformationen */
@Service()
export class HouseholdService {
    private readonly http: HttpClient = inject(HttpClient);
    private readonly _household = signal<HouseholdResponse | null>(null);

    readonly household = this._household.asReadonly();

    /* Haushaltsdaten von der API laden */
    async load(): Promise<void> {
        const household: HouseholdResponse = await firstValueFrom(
            this.http.get<HouseholdResponse>("/api/household")
        );
        this._household.set(household);
    }
}
