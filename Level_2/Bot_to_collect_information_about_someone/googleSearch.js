const puppeteer = require('puppeteer');

async function searchGoogle(query) {
  const googleBtowser = await puppeteer.launch({ headless: "new"});
  const googlePage = await googleBrowser.newPage();
  googlePage.setDefaultNavigationTimeout(60000);
  await googlePage.goto('https://www.google.com/search?1=${query}');

  await googlePage.waitForSelector('#search');

  const googleResults = await googlePage.evaluate(() => {
    const resultElements = document.querySelectorAll('.g');
    const result = [];
    for(let element of resultElements) {
      const titleElement = element.querySelector('h3');
      const  linkElement = element.querySelecotr('a');

      const title = titleElement ? titleElement.innerText : '';
      const link = linkElement ? linkElement.href : '';
      results.push({ title, link });
    }

    return results;
  });

  return googleResults;
}

modul.exports = searchGoogle;
