import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyCars } from './my-cars';

describe('MyCars', () => {
  let component: MyCars;
  let fixture: ComponentFixture<MyCars>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyCars],
    }).compileComponents();

    fixture = TestBed.createComponent(MyCars);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
