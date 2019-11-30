import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { Seller } from '../models/seller';

const TOKEN_KEY = 'shelf.token';

interface TokenResponse {
  access_token: string;
  token_type: string;
  expires_in: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedIn = new BehaviorSubject<boolean>(!!this.token);
  readonly loggedIn$ = this.loggedIn.asObservable();

  constructor(private http: HttpClient) { }

  get token(): string {
    return localStorage.getItem(TOKEN_KEY);
  }

  login(email: string, password: string): Observable<TokenResponse> {
    // OWIN token endpoint wants form encoding, not json
    const body = new HttpParams()
      .set('grant_type', 'password')
      .set('username', email)
      .set('password', password);

    return this.http.post<TokenResponse>(environment.tokenUrl, body.toString(), {
      headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
    }).pipe(
      tap(res => {
        localStorage.setItem(TOKEN_KEY, res.access_token);
        this.loggedIn.next(true);
      })
    );
  }

  register(displayName: string, email: string, password: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/sellers/register`, { displayName, email, password });
  }

  me(): Observable<Seller> {
    return this.http.get<Seller>(`${environment.apiUrl}/sellers/me`);
  }

  updateProfile(displayName: string, bio: string): Observable<Seller> {
    return this.http.put<Seller>(`${environment.apiUrl}/sellers/me`, { displayName, bio });
  }

  logout() {
    localStorage.removeItem(TOKEN_KEY);
    this.loggedIn.next(false);
  }
}
