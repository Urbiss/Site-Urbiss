export class User {
  public constructor(init?: Partial<User>) {
    Object.assign(this, init);
  }

  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
}
