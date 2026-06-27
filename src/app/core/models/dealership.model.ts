export interface Dealership {
  id: number;
  name: string;
  email: string;
  phoneNumber: string;
  address: string;
  city: string;
  state: string;
  country: string;
  employeeCount?: number;
  carCount?: number;
}