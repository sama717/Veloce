import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminCars } from './admin-cars';

describe('AdminCars', () => {
  let component: AdminCars;
  let fixture: ComponentFixture<AdminCars>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCars],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCars);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
