import { Car } from "./car.model";

export interface CreateRentalBookingDto {
  carId: number;
  startDate: string;        
  endDate: string;          
  verificationDocument: string;
}

export interface CreateConsultationBookingDto {
  carId: number;
  dealershipId: number;
  preferredDate: string;  
  notes?: string;
}

export interface UpdateBookingStatusDto {
  status: number;          
}

export interface Booking {
  id: number;
  userId: number;
  carId: number;
  status: 'Pending' | 'Confirmed' | 'Canceled' | 'Completed' | 'Rejected';
  bookingType: 'Rental' | 'Consultation';
  createdAt: string;
  rentalDetail?: RentalDetail;
  consultationDetail?: ConsultationDetail;
  car?: Car; 
  user?: {          
    id: number;
    firstName: string;
    lastName: string;
    email?: string;
  };
}

export interface RentalDetail {
  id: number;
  verificationDocument: string;
  startDate: string;
  endDate: string;
  totalPrice: number;
  stripePaymentIntentId?: string;
}

export interface ConsultationDetail {
  id: number;
  dealershipId: number;
  preferredDate: string;
  notes?: string;
  dealership?: Dealership;
}

export interface CreatePaymentIntentDto {
  bookingId: number;
}

export interface PaymentIntentResponse {
  clientSecret: string;
  paymentIntentId: string;
  amount: number;
  dealershipCut: number;
  ownerPayout: number;
}

export interface ConfirmPaymentDto {
  bookingId: number;
  paymentIntentId: string;
}

export interface Payment {
  id: number;
  rentalDetailId: number;
  amount: number;
  tax: number;
  totalAmount: number;
  dealershipCut?: number;
  ownerPayout?: number;
  status: 'Pending' | 'Cancelled' | 'Failed' | 'Paid' | 'Refunded';
  stripePaymentId: string;
  createdAt: string;
}

export interface RentalContractData {
  bookingId: number;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  carBrand: string;
  carModel: string;
  carYear: number;
  startDate: string;
  endDate: string;
  totalDays: number;
  totalPrice: number;
  depositPaid: number;
  generatedAt: string;
  dealershipName: string;
}

export interface Dealership {
  id: number;
  name: string;
  address: string;
  city: string;
  state: string;
  country: string;
}

export enum BookingStatus {
  Pending = 0,
  Confirmed = 1,
  Canceled = 2,
  Completed = 3,
  Rejected = 4
}

export enum BookingType {
  Rental = 0,
  Consultation = 1
}

export enum PaymentStatus {
  Pending = 0,
  Cancelled = 1,
  Failed = 2,
  Paid = 3,
  Refunded = 4
}