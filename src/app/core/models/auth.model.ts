export interface LoginCredentials {
  identifier: string;
  password: string;
}

export interface RegisterData {
  firstName: string;
  middleName?: string | null;
  lastName: string;
  email: string;
  phoneNumber: string;
  username: string;
  password: string;
  mode: number; // 0 = Customer, 1 = Provider
}

export interface AuthResponse {
  id: number;
  firstName: string;
  middleName?: string | null;
  lastName: string;
  username: string;
  email: string;
  profilePicture?: string | null;
  role: string;
  isEmailVerified: boolean;
  token?: string;              // ✅ Add this
  refreshToken?: string; 
  clientProfile?: {
    id: number;
    userMode: string;
  } | null;
  employeeProfile?: {
    id: number;
    dealershipId: number;
    position: string;
    dealershipName: string;
  } | null;
}

export interface ForgotPasswordData {
  email: string
}

export interface ResetPasswordData {
  token: string;
  newPassword: string;
  confirmPassword: string;
}