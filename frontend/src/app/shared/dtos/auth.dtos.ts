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