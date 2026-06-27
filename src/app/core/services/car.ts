import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Car, CarFilterParams } from '../../core/models/car.model';

@Injectable({
  providedIn: 'root',
})
export class CarService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7012/api/Car';

  // No mapping – just return the API data as-is
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

  getMyCars(): Observable<Car[]> {
    return this.http.get<Car[]>(`${this.apiUrl}/my-cars`);
  }

  getCarsByDealership(dealershipId: number): Observable<Car[]> {
    return this.http.get<Car[]>(`${this.apiUrl}?dealershipId=${dealershipId}`);
  }

  createCar(data: FormData): Observable<Car> {
    return this.http.post<Car>(this.apiUrl, data);
  }

  updateCar(id: number, data: FormData): Observable<Car> {
    return this.http.put<Car>(`${this.apiUrl}/${id}`, data);
  }

  deleteCar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  uploadImages(carId: number, images: File[]): Observable<Car> {
    const formData = new FormData();
    images.forEach((file) => formData.append('images', file));
    return this.http.post<Car>(`${this.apiUrl}/${carId}/images`, formData);
  }

  deleteImage(carId: number, imageId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${carId}/images/${imageId}`);
  }

  setMainImage(carId: number, imageId: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${carId}/images/${imageId}/main`, {});
  }

  reorderImages(carId: number, imageIds: number[]): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${carId}/images/reorder`, imageIds);
  }
}