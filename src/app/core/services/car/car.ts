import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Car, CarFilterParams } from '../../models/car.model'; 

@Injectable({
  providedIn: 'root'
})
export class CarService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7012/api/Car';

  getCars(filters: CarFilterParams): Observable<Car[]> {
    let params = new HttpParams();

    Object.entries(filters).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, value.toString());
      }
    });

    return this.http.get<Car[]>(this.apiUrl, { params });
  }

  getCarById(id: number): Observable<Car> {
    return this.http.get<Car>(`${this.apiUrl}/${id}`);
  }
}