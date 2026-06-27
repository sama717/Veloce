export interface Car {
  id: number;
  brand: string;
  model: string;
  year: number;
  color: string;
  mileage: number;
  seats: number;
  description?: string;
  price?: number;
  pricePerDay?: number;
  quantity: number;
  availableQuantity: number;
  type: string;          // "Sale" or "Rent" (from API)
  status: string;        // "Available", "Rented", etc.
  condition: string;     // "New" or "Used"
  imageUrls: string[];
  createdAt: string;
}

export interface CarImage {
  id: number;
  imageUrl: string;
  isMain: boolean;
  displayOrder?: number;
}

export interface CarFilterParams {
  brand?: string;
  model?: string;
  color?: string;
  condition?: number;     
  yearFrom?: number;
  yearTo?: number;
  minPrice?: number;
  maxPrice?: number;
  type?: number;          
  ownerId?: number;
  dealershipId?: number;
}
