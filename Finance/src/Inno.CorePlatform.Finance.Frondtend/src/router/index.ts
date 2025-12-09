import { RouteRecordRaw } from 'vue-router';
export const routes: Array<RouteRecordRaw> = [
  {
    path: '/crud',
    name: 'crud',
    meta: {
      name: '选项式列表'
    },
    component: () => import('@/views/crud/index.vue')
  }
].map((i) => {
  if (i.component instanceof Function) {
    return {
      ...i,
      component: async () => {
        const a = await i.component();
        return { ...a.default, name: i.name };
      }
    };
  }
  return i;
});
