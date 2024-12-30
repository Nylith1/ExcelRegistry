import { ReactNode } from "react";

export interface ReactNodeProps {
  children: ReactNode;
}

export interface UserDto {
  id: string;
  name: string;
  email: string;
  profilePhoto: string;
  userRoleNames: string[];
}

export interface UserRoleDto {
  id: string;
  name: string;
}

export interface RegentDto {
  id: string;
  indexNumber: string;
  date: Date;
  solutionBeingPrepared: string;
  dateOfManufacture: Date;
  expireDate: Date;
  user: string;
}
