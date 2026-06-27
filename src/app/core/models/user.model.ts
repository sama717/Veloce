export interface UserProfile {
  id: number;
  firstName: string;
  middleName?: string | null;
  lastName: string;
  email: string;
  phoneNumber: string;
  username: string;
  role: string;
  isEmailVerified: boolean; 
  profilePicture?: string | null;
  createdAt: string;
  clientProfile?: ClientProfile | null;
  employeeProfile?: EmployeeProfile | null;
}

export interface ClientProfile {
  id: number;
  userMode: string;
}

export interface EmployeeProfile {
  id: number;
  position: string;
  dealershipId: number;
  dealershipName: string;
}

export interface UpdateUserProfileDto {
  firstName?: string;
  middleName?: string;
  lastName?: string;
}

export interface UpdateProfilePictureDto {
  profilePicture: File;
}