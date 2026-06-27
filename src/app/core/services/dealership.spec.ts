import { TestBed } from '@angular/core/testing';

import { Dealership } from './dealership';

describe('Dealership', () => {
  let service: Dealership;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Dealership);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
