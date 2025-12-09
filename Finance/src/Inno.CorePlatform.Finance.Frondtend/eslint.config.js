import eslintrcDefault from '@inno/inno-mc-vue3/eslintrcDefault';
import _ from 'lodash';

export default [
  ...eslintrcDefault,
  {
    languageOptions: {
      globals: {
        node: true
      }
    },
    ignores: ['**/node_modules/**', '**/dist/**'],
    rules: {}
  }
];
