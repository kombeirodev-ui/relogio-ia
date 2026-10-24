import React from 'react';

const segments = {
  a: '5,1 23,1 26,4 23,7 5,7 2,4', b: '24,7 27,4 27,21 24,24 21,21 21,10',
  c: '24,25 27,28 27,44 24,47 21,44 21,28', d: '5,42 22,42 25,45 22,48 5,48 2,45',
  e: '3,25 6,28 6,40 3,44 0,41 0,28', f: '3,5 6,8 6,20 3,23 0,20 0,8', g: '5,21 22,21 25,24 22,27 5,27 2,24',
};
const digits = ['abcdef', 'bc', 'abdeg', 'abcdg', 'bcfg', 'acdfg', 'acdefg', 'abc', 'abcdefg', 'abcdfg'];

export default function ClockDigits({ time, style, atlas }) {
  if (style === 'atlas' && atlas) return <span className="atlas-clock" aria-label={time}>{[...time].map((char, index) => <span key={index} aria-hidden="true" className="atlas-digit" style={{ backgroundImage: `url("${atlas}")`, backgroundPosition: `${(char === ':' ? 10 : Number(char)) * 10}% 50%` }} />)}</span>;
  if (style === 'segments') return <span className="segment-clock" aria-label={time}>{[...time].map((char, index) => char === ':' ? <svg key={index} className="segment-colon" viewBox="0 0 10 48" aria-hidden="true"><rect x="3" y="13" width="4" height="5" /><rect x="3" y="31" width="4" height="5" /></svg> : <svg key={index} viewBox="0 0 28 49" aria-hidden="true">{Object.entries(segments).map(([id, points]) => <polygon key={id} points={points} opacity={digits[Number(char)]?.includes(id) ? 1 : 0.07} />)}</svg>)}</span>;
  return <span className={style === 'outline' ? 'outline-clock' : ''}>{time}</span>;
}
