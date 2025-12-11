---
title: "استفاده از zustand برای بهبود مدیریت state ها"
date: "2025-12-05"
author: "فرشید"
image: "/images/blog/zustand.webp"
---

## چرا zustand ؟

وقتی صحبت از مدیریت stateها در اپلیکیشن‌های react ای میشه نمیشه یه نسخه برای همه اپلیکیشن‌ها پیچید، بعضی وقت ها stick شدن به همون امکانات react مثل Context API جوابه و بعضی وقتها نیاز به کتابخانونه هایی مجزا برای مدیریت stateها داریم. درباره اینکه استراتژی مناسب برای هر موضوع چیه، امیدوارم بتونیم توی یه پست مجزا راجع بهش صحبت می کنیم ولی به عنوان یه rule of thumb باید بگم، این دو نکته می تونه به شما بگه که شاید وقتش باشه که به یک کتابخونه مدیریت state مجزا فکر کنید.

-   **تعداد زیاد state ها در Context API**
-   **نرخ بالای بروزرسانی state ها در کامپوننت**

البته core react دائما در حال توسعه است و مواردی مثل react compiler می تونه تا حدودی شرایط رو تغییر بده.

حالا بریم سر داستان خودمون، zustand قطعا توی دسته کتابخونه های مجزا از core react برای مدیریت stateها قرار می گیره، کار با zustand خیلی راحت و مشابه react عه، اول از همه بگذارید راجع به فلسفه کارکردی zustand یکم صحبت کنیم و اینکه چجوری داره کار می‌کنه؟

## فلسفه عملکردی zustand

وقتی شما با zustand یک store در جایی می سازی در حقیقت در run-time جاوااسکریپت یه محدوده ای می‌سازی که داخلش چیزهایی که می‌خوای رو ذخیره می‌کنی. یعنی یه چیزی شبیه این شکل زیری

![Zustand run-time assets](/images/assets/zustand1.webp)

اول بگم ‌lifecycle این store در وهله اول کاملا به ‌‌‌run-time مرورگر شما بستگی داره یعنی اگر صفحه مرورگر رو reload کنید قاعدتا این store پاک میشه مگر اینکه جای دیگه ای هم persist بشه که موضوع بحث ما فعلا نیست.
همونطور که توی شکل بالام مشخصه اپلیکیشن شما هر حرکتی بزنه دخلی(!) به این store به طور معمول نداره چون space یا فضای حافظه و run-time شون جداست.

حالا از این ببعدش جالب میشه، شما وقتی zustand رو به طور مثال در برنامه react تون استفاده می‌کنید یه wrapper ای از جنس stateهای react دورش ایجاد میشه که فرآیندهای آپدیت و rendering در react بدین وسیله handle میشه.

شکل زیر تقریبی از چیزی است که اتفاق می‌افته. به عبارتی برای اینکه react برای این که بتونه فرآیندهای rendering رو برعهده بگیره نیاز به چیزی داره که براش معنا و مفهوم داشته باشه که همون stateهاست. حالا ما هربار که useStore رو استفاده کنیم(بعدا خواهیم گفت)، در حقیقت کنترل re-render و render رو برعهده می‌گیریم.

![Zustand React model](/images/assets/zustand2.webp)

## اولین گام zustand - ساخت store

Zustand برای ساختن state کافیه متد `create` رو صدا بزنید:

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

همونطور که می‌بینید تابع `‌create` یه تابع به عنوان آرگومان خودش می‌گیره. این تابع یک خودش یه آرگومان به نام `‌‌set` داره که می‌تونیم ازش برای مقداردهی موارد داخل ‌store مون استفاده کنیم. (آرگومان `get` هم داره که بعدا میگیم به چه دردی میخوره)

طبعا ما می تونیم state ها رو به صورت پیچیده تری آپدیت کنیم و دستمون بازه اینجا. به این سبک partial update هم میگن.

```typescript
setCurrentView: (newView: TasksView) => set({ currentView: newView, tasks: [] }),
```

همچنین تابع بالا یه آرگومان دوم هم می گیره که باهاش می تونیم کل store رو replace کنیم، مثلا در مثال زیر store مون رو می تونیم با `clear` ریست کنیم. البته در استفاده ازش با دقت عمل کنید چون می تونه کل store تون رو wipe کنه.

```typescript
const initialState = {
  user: null,
  loggedIn: false,
};

const useStore = create((set) => ({
  ...initialState,
  clear: () => set(initialState, true),
}));
‍‍‍‍‍‍‍‍‍
```

خب حالا بریم این custom hook ای که ساختیم رو استفاده کنیم. منظورم همین store هست که با useStore ایجاد شده است. برای این کار روش های متنوعی وجود داره و خیلی راحت می‌تونید انتخاب اشتباهی رو صورت بدید و اوضاع ‌‌رو خرابش کنید. ما در ادامه خیلی گاماس گاماس با هم پیش میریم تا معایب و مزایای هر روش رو به خوبی خودتون متوجه بشید.

## استفاده از zustand در کامپوننت

### استفاده از الگوی atmoic تا ابد!

استفاده از zustand بدین شکل که state ها رو در متغیرهای مجزا استخراج کنیم(`atomic pattern`)، همیشه جوابه و توصیه خود zustand هم در غالب مواقع همین روشه.

```typescript
const currentView = useTasksStore(state => state.currentView);
const tasks = useTasksStore(state => state.tasks);
const currentFilter = useTasksStore(state => state.currentFilter);
‍‍‍
```

همونطور که واضحه در این حالت و هربار باید explictly بگیم چی ها رو می خواهیم. یعنی باید از selector استفاده کنیم، مشابه اونچه در غالب state manager ها مثل redux هم داشتیم. اینجوری دقیقا میگیم چیارو می خواهیم و فقط state مربوط به همونا میاد توی کامپوننت مون و هربار که فقط این state عوض بشه کامپوننت ما rerender میشه. یعنی میشه با تقریبی اینو معادل این دونست برای درک بهتر وگرنه در واقعیت کمی پیچیده تره

```typescript
const [currentFilter] = useState({"Current State's Value in the Store"});
.
.
```

خب دیگه حالا هرچی توی store عوض بشه تا وقتی currentFilter عوض نشه این state موجبات rerender کامپوننت مون رو فراهم نمی کنه و این درسته!

### خواندن store به صورت گرتره‌ای

می تونیم به روش زیر state مون رو از store بخونیم که خب اتفاقا کار هم می کنه.

```typescript
const { currentFilter } = useTasksStore();
```

‌‌zustand به طور پیشفرض در جواب `useTasksStore` کل store رو برمی گردونه که در واقع اگر به فلسفه zustand برگردیم یعنی stateهای کل store در این کامپوننت قرار داده میشن و این یعنی اگر هرکدوم از اون state ها آپدیت بشن این کامپوننت rerender خواهد شد. در این مورد object destructring هم با این که انجام شده ولی تاثیری نداره.
این اتفاق شبیه همون چیزی که توی Context API خود react رخ میده، فلذا اینگونه استفاده با هدف zustand در غالب مواقع همخوانی نداره.

> خب تا همینجا کافیه که بتونید توی کامپوننت‌هاتون از zustand استفاده کنید و کنترل ‌‌rendering رو راحتتر داشته باشید. از اینجا ببعد ما به این می‌پردازیم که چگونه از ‌zustand به صورت هوشمندانه‌تری استفاده کنیم و اینم یادتون نره که هرجا شک کردید که کدوم راه درستتره just stick to atomic pattern که همیشه جوابه!

> استفاده از zustand طبیعتا هیچ محدودیتی در عدم استفاده از کتابخونه‌های دیگه مدیریت ‌state برای شما ایجاد نمی کنه و می تونید توی یه پروژه redux ای یا با context API به کارش بگیرید، اگر لازم بود.

## استفاده هوشمندانه‌تر از ‌zustand

اول از همه با یه مثال react ای بگم چه مواقع الگوی atomic کد مارو یه جوری می‌کنه!

```typescript
import { create } from "zustand";

type State = {
    count: number;
    user: { name: string; email: string };
    increment: () => void;
};

export const useStore = create<State>((set) => ({
    count: 0,
    user: { name: "Marco", email: "marco@example.com" },
    increment: () => set((state) => ({ count: state.count + 1 })),
}));
```

حالا استفاده ازش

```typescript
‍‍import { useStore } from './store'

export default function Example() {
  console.log('rendered')

  const { user } = useStore((state) => ({
    user: state.user,
  }))

  return (
    <div>
      <p>Count: {count}</p>
      <p>User: {user.name}</p>
    </div>
  )
}
```

در این مثال اگر فقط مقدار email در آبجکت user در جایی عوض بشه این کامپوننت rerender میشه. خب قاعدتا این مطلوب نیست و ما باید به صورت atomic این رو بخونیم. یعنی اینجوری بنویسیم.

‍‍‍```typescript
const name = useStore(state => state.user.name);
const email = useStore(state => state.user.email);

````

 خب این آبجکت دوتا key داشت اگر تعداد key های بیشتری میداشت باید کلی از اینا می‌نوشتیم.

 ‍```typescript
const name = useStore(state => state.user.name);
const email = useStore(state => state.user.email);
const firstname = useStore(state => state.user.firstname);
const lastname = useStore(state => state.user.lastname);
const nickname = useStore(state => state.user.nickname);

````

خب اینجوری یکم boilerplate میشه و جذاب نیست. به جای این کار می تونیم از مفهوم shallow استفاده کنیم.

### استفاده از shallow و useShallow

خب اول از همه مروری به مبحث shallow equality خواهیم کرد. چک می‌کنیم فقط **لایه اول آبجکت یا آرایه** یکیه یا نه، عمیق نمی‌ریم داخلش.

```js
function shallowEqual(a, b) {
  if (a === b) return true;          // اگه همون مرجع باشه
  if (!a || !b) return false;        // اگه یکی null یا undefined باشه
  if (Object.keys(a).length !== Object.keys(b).length) return false;

  for (let key in a) {
    if (a[key] !== b[key]) return false; // فقط لایه اول رو چک می‌کنیم
  }
  return true;
}

// مثال:
shallowEqual({x:1, y:2}, {x:1, y:2}) // true
shallowEqual({x:1}, {x:1, y:2})      // false
‍‍‍
```

در مقابل shallow equality مفهوم deep equality قرار داره که میره تا فیها خالدون رو بررسی می کنه که برابر باشند. تقریبا تمامی سیستم‌های مدیریت ‌state ازجمله خود react و zustand برای اینکه متوجه بشن state عوض شده یا نه از shallow استفاده می کنند که همونطور که فانکشنش رو می بینید خیلی سریعتر از حالتی است که بخواد تا آخرین لایه آبجکت های nested رو بررسی کنه ولی خب یه سری چیزها رو هم از دست میده یعنی یه سری تغییرات رو چون shallow مقایسه می کنه sense نمی کنه.

## منابع

-   [Zustand Documentation](https://zustand.docs.pmnd.rs/)
-   [Introducing Zustand](https://frontendmasters.com/blog/introducing-zustand/)
-   [Why Zustand has useShallow](https://www.mordonez.me/posts/why-zustand-has-useshallow-and-how-it-prevents-unnecessary-renders/?utm_source=chatgpt.com)
