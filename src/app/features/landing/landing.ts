import { Component } from '@angular/core';
import { ScrollingHero } from '../../components/scrolling-hero/scrolling-hero';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [ScrollingHero],
  templateUrl: './landing.html',
  styleUrls: ['./landing.css'],
})
export class Landing {}