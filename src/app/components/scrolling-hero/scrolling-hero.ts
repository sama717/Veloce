import { Component, ElementRef, ViewChild, ViewChildren, QueryList, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import gsap from 'gsap';
import { ScrollTrigger } from 'gsap/ScrollTrigger';

gsap.registerPlugin(ScrollTrigger);

interface Panel {
  screen: string;
  eyebrow: string;
  title: string;
  body: string;
}

@Component({
  selector: 'app-scrolling-hero',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './scrolling-hero.html',
  styleUrls: ['./scrolling-hero.css'],
})
export class ScrollingHero implements AfterViewInit, OnDestroy {
  @ViewChild('sectionRef') sectionRef!: ElementRef<HTMLElement>;
  @ViewChild('phoneStageRef') phoneStageRef!: ElementRef<HTMLDivElement>;
  @ViewChildren('screenImg') screenImgs!: QueryList<ElementRef<HTMLImageElement>>;
  @ViewChildren('panelEl') panelEls!: QueryList<ElementRef<HTMLDivElement>>;

  phoneFrame = '/phone-frame.png';

  panels: Panel[] = [
    {
      screen: '/screens/screen-browse.png',
      eyebrow: 'Browse',
      title: 'Find your car in seconds.',
      body: 'Filter by class, location, and price. Veloce surfaces the best match instantly, no endless scrolling.',
    },
    {
      screen: '/screens/screen-booking.png',
      eyebrow: 'Book',
      title: 'Reserve with a tap.',
      body: 'Pick your dates, confirm insurance, and lock in your booking. No calls, no paperwork, no waiting.',
    },
    {
      screen: '/screens/screen-drive.png',
      eyebrow: 'Drive',
      title: 'Unlock and go.',
      body: 'Your phone is the key. Walk up, unlock, and drive off the moment your rental starts.',
    },
  ];

  private mainTimeline?: gsap.core.Timeline;

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.initAnimation();
    }, 100);
  }

  private initAnimation(): void {
    if (!this.sectionRef || !this.phoneStageRef) return;

    const screens = this.screenImgs.map((ref) => ref.nativeElement);
    const panelElements = this.panelEls.map((ref) => ref.nativeElement);
    const numPanels = this.panels.length;

    if (screens.length === 0 || panelElements.length === 0) return;

    gsap.set(screens, { clearProps: 'all' });
    gsap.set(panelElements, { clearProps: 'all' });

    gsap.set(screens.slice(1), { opacity: 0, scale: 0.95 });
    gsap.set(panelElements.slice(1), { opacity: 0, y: 40 });

    this.mainTimeline = gsap.timeline({
      scrollTrigger: {
        trigger: this.sectionRef.nativeElement,
        start: 'top top',
        end: () => `+=${numPanels * 120}%`,
        pin: this.phoneStageRef.nativeElement,
        pinSpacing: true,
        scrub: 1,
        invalidateOnRefresh: true,
      }
    });

    for (let i = 0; i < numPanels - 1; i++) {
      this.mainTimeline
        .to(panelElements[i], { opacity: 0, y: -40, duration: 0.5 }, `slide-${i}`)
        .to(screens[i], { opacity: 0, scale: 0.95, duration: 0.5 }, `slide-${i}`)
        
        .to(panelElements[i + 1], { opacity: 1, y: 0, duration: 0.5 }, `slide-${i}+=0.3`)
        .to(screens[i + 1], { opacity: 1, scale: 1, duration: 0.5 }, `slide-${i}+=0.3`);
    }
  }

  ngOnDestroy(): void {
    this.mainTimeline?.kill();
    ScrollTrigger.getAll().forEach(t => t.kill());
  }
}