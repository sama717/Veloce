import {
  Component,
  inject,
  OnInit,
  signal,
  computed,
  AfterViewInit,
  ElementRef,
  ViewChild,
  ChangeDetectorRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CarService } from '../../../core/services/car/car';
import { BookingService } from '../../../core/services/booking/booking';
import { Car } from '../../../core/models/car.model';
import {
  CreateRentalBookingDto,
  CreateConsultationBookingDto,
} from '../../../core/models/booking.model';
import { Dealership } from '../../../core/models/booking.model';
import { environment } from '../../../../environments/environment';
import { loadStripe, Stripe, StripeCardElement, StripeElements } from '@stripe/stripe-js';
import { AuthService } from '../../../core/services/auth/auth';

@Component({
  selector: 'app-booking-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './booking-create.html',
  styleUrl: './booking-create.css',
})
export class BookingCreate implements OnInit, AfterViewInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private carService = inject(CarService);
  private bookingService = inject(BookingService);
  private authService = inject(AuthService);
  // private cdr = inject(ChangeDetectorRef);

  @ViewChild('cardElement', { static: false }) cardElement!: ElementRef;

  // Stripe
  stripe: Stripe | null = null;
  elements: StripeElements | null = null;
  card: StripeCardElement | null = null;
  stripeInitialized = signal<boolean>(false);

  // State
  car = signal<Car | null>(null);
  dealerships = signal<Dealership[]>([]);
  isLoading = signal<boolean>(true);
  isSubmitting = signal<boolean>(false);
  isProcessingPayment = signal<boolean>(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  currentBookingId = signal<number | null>(null);

  // Form fields
  selectedDealershipId = signal<number | null>(null);
  startDate = signal<string>('');
  endDate = signal<string>('');
  verificationDocument = signal<string>('');
  preferredDate = signal<string>('');
  notes = signal<string>('');
  bookingType = signal<'Rental' | 'Consultation'>('Rental');

  // Computed
  totalDays = computed(() => {
    const start = this.startDate();
    const end = this.endDate();
    if (!start || !end) return 0;
    const diff = new Date(end).getTime() - new Date(start).getTime();
    return Math.ceil(diff / (1000 * 60 * 60 * 24));
  });

  totalPrice = computed(() => {
    return this.totalDays() * (this.car()?.pricePerDay || 0);
  });

  ngOnInit(): void {
    const carIdStr = this.route.snapshot.paramMap.get('id');
    if (!carIdStr) {
      this.errorMessage.set('Invalid car ID');
      this.isLoading.set(false);
      return;
    }
    this.loadCarDetails(parseInt(carIdStr, 10));
    this.loadDealerships();

    const today = new Date();
    this.startDate.set(today.toISOString().split('T')[0]);
    this.preferredDate.set(today.toISOString().split('T')[0]);
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.endDate.set(tomorrow.toISOString().split('T')[0]);
  }

  async ngAfterViewInit() {
    setTimeout(() => {
      this.initializeStripe();
    }, 200);
  }

  async initializeStripe() {
    if (this.stripeInitialized()) return;
    if (!this.cardElement) {
      console.warn('Card element not found, retrying...');
      setTimeout(() => this.initializeStripe(), 300);
      return;
    }

    this.stripe = await loadStripe(environment.stripePublishableKey);
    if (this.stripe) {
      this.elements = this.stripe.elements();
      this.card = this.elements.create('card', {
        style: {
          base: {
            fontSize: '16px',
            color: '#1E1F1F',
            '::placeholder': { color: '#6B7280' },
          },
        },
      });
      this.card.mount(this.cardElement.nativeElement);
      this.stripeInitialized.set(true);
    } else {
      this.errorMessage.set('Stripe failed to load. Check publishable key.');
    }
  }

  loadCarDetails(id: number): void {
    this.carService.getCarById(id).subscribe({
      next: (data) => {
        this.car.set(data);
        this.bookingType.set(data.type === 'Rent' ? 'Rental' : 'Consultation');
        this.isLoading.set(false);
        setTimeout(() => this.initializeStripe(), 100);
      },
      error: () => {
        this.errorMessage.set('Failed to load car details');
        this.isLoading.set(false);
      },
    });
  }

  loadDealerships(): void {
    this.bookingService.getDealerships().subscribe({
      next: (data) => {
        this.dealerships.set(data);
        if (data.length > 0) {
          this.selectedDealershipId.set(data[0].id);
        }
      },
      error: () => {
        console.error('Failed to load dealerships');
      },
    });
  }

  submitBooking(): void {
    const user = this.authService.currentUser();
    if (!user?.isEmailVerified) {
      this.errorMessage.set(
        'Please verify your email before booking. Check your inbox for the verification link.',
      );
      return;
    }
    const vehicle = this.car();
    if (!vehicle) return;

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    if (this.bookingType() === 'Rental') {
      this.processRentalFlow(vehicle.id);
    } else {
      this.processConsultationFlow(vehicle.id);
    }
  }

  private processRentalFlow(carId: number): void {
    if (!this.startDate() || !this.endDate()) {
      this.errorMessage.set('Please select valid rental dates');
      this.isSubmitting.set(false);
      return;
    }

    if (!this.verificationDocument()) {
      this.errorMessage.set('Please enter your verification ID (license/passport number)');
      this.isSubmitting.set(false);
      return;
    }

    const payload: CreateRentalBookingDto = {
      carId,
      startDate: new Date(this.startDate()).toISOString(),
      endDate: new Date(this.endDate()).toISOString(),
      verificationDocument: this.verificationDocument(),
    };

    this.bookingService.createRental(payload).subscribe({
      next: (booking) => {
        this.currentBookingId.set(booking.id);
        this.processPayment(booking.id);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Failed to create rental booking');
        this.isSubmitting.set(false);
      },
    });
  }

  private processPayment(bookingId: number): void {
    this.isProcessingPayment.set(true);

    this.bookingService.createPaymentIntent(bookingId).subscribe({
      next: async (intentResponse) => {
        if (!this.stripe || !this.card) {
          this.errorMessage.set('Stripe not initialized');
          this.isProcessingPayment.set(false);
          this.isSubmitting.set(false);
          return;
        }

        const { error, paymentIntent } = await this.stripe.confirmCardPayment(
          intentResponse.clientSecret,
          {
            payment_method: {
              card: this.card,
            },
          },
        );

        if (error) {
          this.errorMessage.set(error.message || 'Payment failed');
          this.isProcessingPayment.set(false);
          this.isSubmitting.set(false);
          return;
        }

        if (paymentIntent?.status === 'succeeded') {
          this.bookingService
            .confirmPayment({
              bookingId,
              paymentIntentId: paymentIntent.id,
            })
            .subscribe({
              next: () => {
                this.successMessage.set(`Rental confirmed! Total: $${intentResponse.amount}.`);
                this.isProcessingPayment.set(false);
                this.isSubmitting.set(false);
              },
              error: (err) => {
                this.errorMessage.set(err.error?.message || 'Payment confirmation failed');
                this.isProcessingPayment.set(false);
                this.isSubmitting.set(false);
              },
            });
        }
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Failed to initialize payment');
        this.isProcessingPayment.set(false);
        this.isSubmitting.set(false);
      },
    });
  }

  private processConsultationFlow(carId: number): void {
    if (!this.selectedDealershipId()) {
      this.errorMessage.set('Please select a dealership');
      this.isSubmitting.set(false);
      return;
    }

    if (!this.preferredDate()) {
      this.errorMessage.set('Please select a preferred date');
      this.isSubmitting.set(false);
      return;
    }

    const payload: CreateConsultationBookingDto = {
      carId,
      dealershipId: this.selectedDealershipId()!,
      preferredDate: new Date(this.preferredDate()).toISOString(),
      notes: this.notes() || undefined,
    };

    this.bookingService.createConsultation(payload).subscribe({
      next: () => {
        this.successMessage.set(
          'Consultation scheduled successfully! The dealership will contact you.',
        );
        this.isSubmitting.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Failed to schedule consultation');
        this.isSubmitting.set(false);
      },
    });
  }

  downloadContract(): void {
    const bookingId = this.currentBookingId();
    if (!bookingId) {
      this.errorMessage.set('No booking found to download contract.');
      return;
    }

    this.bookingService.getRentalContract(bookingId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `RentalContract_${bookingId}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.errorMessage.set('Failed to download contract.');
      },
    });
  }

  cancel(): void {
    this.router.navigate(['/catalog']);
  }
}
