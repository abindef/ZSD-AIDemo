// @ts-check
import { defineStore, acceptHMRUpdate } from 'pinia';

export const useTestStore = defineStore('test', {
  state: () => ({
    name: '',
    isAdmin: true
  }),

  actions: {
    test() {
      this.$patch({
        name: 'test'
      });
    }
  }
});

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useTestStore, import.meta.hot));
}
