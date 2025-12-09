---
title: "استفاده از zustand برای بهبود مدیریت state ها"
date: "2025-12-05"
author: "فرشید"
image: "/images/blog/zustand.webp"
---

## چرا zustand ؟

وقتی صحبت از مدیریت stateها در اپلیکیشن‌های react ای میشه نمیشه یه نسخه برای همه اپلیکیشن‌ها پیچید، بعضی وقت ها stick شدن به همون امکانات react مثل Context API جوابه و بعضی وقتها نیاز به کتابخانون هایی مجزا برای مدیریت stateها داریم. درباره اینکه استراتژی مناسب برای این موضع چیه، انشالله توی یه پست مجزا صحبت می کنیم ولی به عنوان یه rule of thumb باید بگم، این دو نکته می تونه به شما بگه که شاید وقتش باشه که به یک کتابخونه مدیریت state مجزا فکر کنید.

-   **تعداد زیاد state ها در Context API**
-   **نرخ بالای بروزرسانی state ها در کامپوننت**

البته core react دائما در حال توسعه است و مواردی مثل react compiler می تونه تا حدودی شرایط رو تغییر بده.

خب zustand قطعا توی دسته کتابخونه های بیرونی قرار می گیره، کار با zustand خیلی راحت و مشابه react عه، اول از همه بگذارید راجع به فلسفه کارکردی zustand یکم صحبت کنیم و اینکه چجوری داره کار می‌کنه؟

## فلسفه عملکردی zustand

وقتی شما با zustand یک store در جایی می سازی در حقیقت در run-time جاوااسکریپت یه محدوده ای می‌سازی که داخلش چیزهایی که می‌خوای رو ذخیره می‌کنی. یعنی یه چیزی شبیه این شکل زیری

![Zustand run-time assets](/images/assets/zustand1.webp)

اول بگم ‌lifecycle این store کاملا به ‌‌‌run-time شما بستگی داره یعنی اگر صفحه رو reload کنید قاعدتا این store پاک میشه مگر اینکه جای دیگه ای هم persist بشه که موضوع بحث ما فعلا نیست.
همونطور که توی شکل بالام مشخصه اپلیکیشن شما هر حرکتی بزنه دخلی(!) به این store به طور معمول نداره جون space یا فضای حافظه و run-time شون جداست.

حالا از این ببعدش جالب میشه، شما وقتی zustand رو به طور مثال در برنامه react تون استفاده می‌کنید یه wrapper ای از جنس stateهای react دورش ایجاد میشه که فرآیندهای آپدیت و rendering در react بدین وسیله handle میشه.

شکل زیر تقریبی از چیزی است که اتفاق می‌افته. به عبارتی برای اینکه react بتونه rendering رو برعهده بگیره نیازی به چیزی که براش آشناست یعنی همون stateها داره. حالا ما هربار که useStore رو استفاده کنیم(بعدا خواهیم گفت)، در حقیقت کنترل re-render و render رو برعهده می‌گیریم.

![Zustand React model](/images/assets/zustand2.webp)

## اولین گام zustand

Zustand یه کتابخونهٔ مدیریت state کم‌حجم ولی واقعاً پرقدرتِ. برای ساختن state کافیه متد `create` رو صدا بزنید:

```javascript
import { create } from "zustand";
```

حالا store رو populate می‌کنیم:

```typescript
export const useTasksStore = create<TasksState>((set) => ({
    tasks,
    setTasks: (arg: Task[] | ((tasks: Task[]) => Task[])) => {
        set((state) => {
            return {
                tasks: typeof arg === "function" ? arg(state.tasks) : arg,
            };
        });
    },
    currentView: "list",
    setCurrentView: (newView: TasksView) => set({ currentView: newView }),
    currentFilter: "",
    setCurrentFilter: (newFilter: string) => set({ currentFilter: newFilter }),
}));
```

همونطور که می‌بینید تابع ‌create یه تابع به عنوان آرگومان خودش می‌گیره. این تابع یک ‌‌set داره که می‌تونیم ازش برای ‌set موارد داخل ‌store مون استفاده کنیم.

## منابع

-   [Introducing Zustand](https://frontendmasters.com/blog/introducing-zustand/)
-   [Zustand Documentation](https://zustand.docs.pmnd.rs/)
