/* Antwort nach erfolgreicher Registrierung oder Anmeldung */
export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
}

/* Anfrage zur Anmeldung eines Benutzers */
export interface LoginRequest{
    email: string;
    password: string;
}

/* Anfrage zur Registrierung eines neuen Benutzers */
export interface RegisterRequest {
    displayName: string;
    email: string;
    password: string;
    inviteCode?: string;
}