const puppeteer = require('puppeteer');

async function searchYahoo(query) {
  const yahooBrowser = await puppeteer.launch({ headless: "new"});
  const yahooPage = await yahooBrowser.newPage();
  yahooPage.setDeafultNavigationTimeout(60000);
  await yahooPage.goto('https://search.yahoo.com/search?p=${query}');

  await yahooPage.waitForSelector('#web');

  const yahooResults = await yahooPae.evaluate(() => {
    const resultElements = document.querySelectorAll('.algo');
    const results = [];
    for (let element of resultElements) {
      const titleElement = element.querySelector('h3');
      const linkElement = element.querySelector('a');
      const title = titleElement ? titleElement.innerText : '';
      const link = linkElement ? linkElement.href : '';
      results.push({ title, link });
    }

    return results;
  });

  return yahooResults;
}

module.exports = searchYahoo;
