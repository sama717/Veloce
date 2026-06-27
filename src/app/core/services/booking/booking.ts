import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  CreateRentalBookingDto,
  CreateConsultationBookingDto,
  Booking,
  UpdateBookingStatusDto,
  PaymentIntentResponse,
  ConfirmPaymentDto,
  RentalContractData
} from '../../models/booking.model';
import { Dealership } from '../../models/booking.model';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7012/api';

  getDealerships(): Observable<Dealership[]> {
    return this.http.get<Dealership[]>(`${this.apiUrl}/Dealership`);
  }

  // ---------- BOOKINGS ----------
  createRental(data: CreateRentalBookingDto): Observable<Booking> {
    return this.http.post<Booking>(`${this.apiUrl}/Booking/rental`, data);
  }

  createConsultation(data: CreateConsultationBookingDto): Observable<Booking> {
    return this.http.post<Booking>(`${this.apiUrl}/Booking/consultation`, data);
  }

  getBooking(id: number): Observable<Booking> {
    return this.http.get<Booking>(`${this.apiUrl}/Booking/${id}`);
  }

  getUserBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.apiUrl}/Booking/user`);
  }

  getCarBookings(carId: number): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.apiUrl}/Booking/car/${carId}`);
  }

  updateBookingStatus(id: number, data: UpdateBookingStatusDto): Observable<Booking> {
    return this.http.put<Booking>(`${this.apiUrl}/Booking/${id}/status`, data);
  }

  cancelBooking(id: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/Booking/${id}/cancel`, {});
  }

  deleteBooking(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Booking/${id}`);
  }

  createPaymentIntent(bookingId: number): Observable<PaymentIntentResponse> {
    return this.http.post<PaymentIntentResponse>(
      `${this.apiUrl}/Payment/create-payment-intent`, 
      { bookingId }
    );
  }

  confirmPayment(data: ConfirmPaymentDto): Observable<{ success: boolean; message: string }> {
    return this.http.post<{ success: boolean; message: string }>(
      `${this.apiUrl}/Payment/confirm-payment`, 
      data
    );
  }

  getRentalContract(bookingId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/RentalContract/${bookingId}`, { 
      responseType: 'blob'
    });
  }

  getRentalContractData(bookingId: number): Observable<RentalContractData> {
    return this.http.get<RentalContractData>(`${this.apiUrl}/RentalContract/${bookingId}/data`);
  }

  getProviderBookings(): Observable<Booking[]> {
  return this.http.get<Booking[]>(`${this.apiUrl}/Booking/provider`);
}
}