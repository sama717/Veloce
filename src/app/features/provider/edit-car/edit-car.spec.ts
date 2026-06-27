import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCar } from './edit-car';

describe('EditCar', () => {
  let component: EditCar;
  let fixture: ComponentFixture<EditCar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditCar],
    }).compileComponents();

    fixture = TestBed.createComponent(EditCar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
