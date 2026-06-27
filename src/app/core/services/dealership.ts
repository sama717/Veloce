import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Dealership } from '../../core/models/dealership.model';

@Injectable({
  providedIn: 'root',
})
export class DealershipService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7012/api/Dealership';

  getDealership(id: number): Observable<Dealership> {
    return this.http.get<Dealership>(`${this.apiUrl}/${id}`);
  }

  getDealerships(): Observable<Dealership[]> {
    return this.http.get<Dealership[]>(this.apiUrl);
  }
}
