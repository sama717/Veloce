import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminDealerships } from './admin-dealerships';

describe('AdminDealerships', () => {
  let component: AdminDealerships;
  let fixture: ComponentFixture<AdminDealerships>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminDealerships],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDealerships);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
